"""Read selected PDF pages locally; stdout is reserved for the JSON response."""
import base64
import contextlib
import io
import json
import math
import sys

MAX_PAGES = 10
MAX_FILE_BYTES = 5 * 1024 * 1024


def read_pages(request):
    import pypdfium2 as pdfium
    from rapidocr import RapidOCR

    content = base64.b64decode(request["pdf"], validate=True)
    if len(content) > MAX_FILE_BYTES:
        raise ValueError("PDF exceeds the file limit")
    pages = request["pages"]
    if not pages or len(pages) > MAX_PAGES or len(set(pages)) != len(pages):
        raise ValueError("Invalid page selection")
    engine = RapidOCR(params={"EngineConfig.onnxruntime.intra_op_num_threads": 2, "EngineConfig.onnxruntime.inter_op_num_threads": 1})
    texts = {}
    with contextlib.closing(pdfium.PdfDocument(content)) as document:
        if len(document) > MAX_PAGES:
            raise ValueError("PDF exceeds the page limit")
        for index in pages:
            if type(index) is not int or index < 0 or index >= len(document):
                raise ValueError("Invalid page index")
            with contextlib.closing(document[index]) as page:
                width, height = page.get_size()
                if not all(math.isfinite(value) and value > 0 for value in (width, height)):
                    raise ValueError("Invalid page dimensions")
                # Bound raster memory while retaining useful scan resolution.
                scale = min(3.0, 2400 / max(width, height))
                bitmap = page.render(scale=scale)
                try:
                    image = bitmap.to_pil().convert("RGB")
                    try:
                        result = engine(image)
                    finally:
                        image.close()
                finally:
                    bitmap.close()
            texts[str(index)] = "\n".join(text for text, score in zip(result.txts if result.txts is not None else [], result.scores if result.scores is not None else []) if score >= 0.5)[:200000]
    return texts


if __name__ == "__main__":
    try:
        # Imports/inference may print diagnostics; never mix these with JSON.
        request = json.loads(sys.stdin.buffer.read(8 * 1024 * 1024))
        with contextlib.redirect_stdout(io.StringIO()):
            output = read_pages(request)
        sys.stdout.buffer.write(json.dumps(output, ensure_ascii=False).encode("utf-8"))
    except Exception as error:
        # Do not emit document text or credentials into backend logs.
        print(type(error).__name__, file=sys.stderr)
        sys.exit(1)
