"""Integration tests: real OCR on an image-only PDF, without database access."""
import base64
import json
from pathlib import Path
import subprocess
import sys
import unittest
from io import BytesIO

from PIL import Image, ImageDraw, ImageFont


class PdfOcrTests(unittest.TestCase):
    def request(self, image, pages):
        content = BytesIO()
        image.save(content, format="PDF", resolution=150)
        result = subprocess.run(
            [sys.executable, str(Path(__file__).with_name("read_pdf.py"))],
            input=json.dumps({"pdf": base64.b64encode(content.getvalue()).decode(), "pages": pages}).encode(),
            capture_output=True, timeout=120,
        )
        return result

    def test_recognises_image_only_cv(self):
        image = Image.new("RGB", (1600, 2000), "white")
        font_path = "C:/Windows/Fonts/arial.ttf" if sys.platform == "win32" else "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"
        font = ImageFont.truetype(font_path, 42)
        draw = ImageDraw.Draw(image)
        for index, line in enumerate([
            "JANE SMITH", "SOFTWARE DEVELOPER", "Skills: Python, SQL, JavaScript",
            "Experience building web applications", "Education: Bachelor of Computer Science",
        ]):
            draw.text((100, 100 + index * 100), line, font=font, fill="black")
        result = self.request(image, [0])
        self.assertEqual(result.returncode, 0, result.stderr.decode())
        text = json.loads(result.stdout)["0"].lower()
        self.assertIn("software", text)
        self.assertIn("python", text)
        self.assertGreater(len(text), 50)

    def test_rejects_out_of_range_page(self):
        result = self.request(Image.new("RGB", (400, 400), "white"), [1])
        self.assertNotEqual(result.returncode, 0)
        self.assertEqual(result.stdout, b"")


if __name__ == "__main__":
    unittest.main()
