const {test} = require('node:test');
const assert = require('node:assert/strict');
const load = require('./load-module.cjs');
const {apiRequest} = load('api/client.ts');

test('analysis streams decode split messages and only finish with a result', async () => {
    const oldFetch = global.fetch, oldUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
    process.env.NEXT_PUBLIC_API_BASE_URL = 'https://localhost:7286';
    try {
        let controller;
        global.fetch = async (_url, options) => {
            assert.equal(options.headers.get('Accept'), 'application/x-ndjson');
            assert.equal(options.headers.get('Authorization'), 'Bearer jwt');
            return new Response(new ReadableStream({start(c) {controller = c;}}), {headers: {'Content-Type': 'application/x-ndjson'}});
        };
        const progress = [];
        const pending = apiRequest('/api/transcripts/analyse', {token: 'jwt', onProgress: value => progress.push(value)});
        await new Promise(resolve => setImmediate(resolve));
        const send = text => controller.enqueue(new TextEncoder().encode(text));
        send('{"type":"progress","percent":15}\n{"type":"prog');
        await new Promise(resolve => setImmediate(resolve));
        assert.deepEqual(progress, [15]);
        send('ress","percent":65}\n{"type":"result","data":{"averageMark":82}}\n');
        controller.close();
        assert.deepEqual(await pending, {averageMark: 82});
        assert.deepEqual(progress, [15, 65]);
        for (const [body, message] of [
            ['{"type":"progress","percent":25}\n', /closed before results/],
            ['{"type":"error","status":400,"details":{"message":"Unreadable scan"}}\n', /Unreadable scan/],
        ]) {
            global.fetch = async () => new Response(body, {headers: {'Content-Type': 'application/x-ndjson'}});
            await assert.rejects(apiRequest('/api/transcripts/analyse', {onProgress() {}}), message);
        }
    } finally {
        global.fetch = oldFetch;
        if (oldUrl === undefined) delete process.env.NEXT_PUBLIC_API_BASE_URL;
        else process.env.NEXT_PUBLIC_API_BASE_URL = oldUrl;
    }
});
