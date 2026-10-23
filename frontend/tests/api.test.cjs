const {test} = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const ts = require('typescript');
function load(relative, dependencies = {}) {
    const source = fs.readFileSync(path.join(__dirname, '../src', relative), 'utf8');
    const compiled = ts.transpileModule(source, {compilerOptions: {module: ts.ModuleKind.CommonJS, target: ts.ScriptTarget.ES2022}}).outputText;
    const module = {exports: {}};
    new Function('require', 'module', 'exports', compiled)(name => dependencies[name] || require(name), module, module.exports);
    return module.exports;
}
const {apiRequest, ApiError} = load('api/client.ts');
test('API transport and CV endpoint contracts', async t => {
    const originalFetch = global.fetch;
    const originalUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
    process.env.NEXT_PUBLIC_API_BASE_URL = 'https://localhost:7286/';
    try {
        await t.test('JSON requests join paths and send bearer tokens', async () => {
            global.fetch = async (url, options) => {
                assert.equal(url, 'https://localhost:7286/api/auth/login');
                assert.equal(options.method, 'POST');
                assert.equal(options.headers.get('Authorization'), 'Bearer test-token');
                assert.equal(options.headers.get('Content-Type'), 'application/json');
                assert.deepEqual(JSON.parse(options.body), {email: 'user@example.com', password: 'password1'});
                return Response.json({token: 'jwt'});
            };
            assert.deepEqual(await apiRequest('/api/auth/login', {method: 'POST', token: 'test-token', body: {email: 'user@example.com', password: 'password1'}}), {token: 'jwt'});
        });
        await t.test('multipart files preserve the browser boundary', async () => {
            const form = new FormData();
            form.append('file', new Blob(['%PDF']), 'cv.pdf');
            global.fetch = async (_url, options) => {
                assert.equal(options.body, form);
                assert.equal(options.headers.has('Content-Type'), false);
                return Response.json({cvid: 'cv-id'}, {status: 201});
            };
            assert.deepEqual(await apiRequest('/api/cvs', {method: 'POST', body: form, headers: {'Content-Type': 'application/json'}}), {cvid: 'cv-id'});
        });
        await t.test('empty 204 succeeds', async () => {
            global.fetch = async () => new Response(null, {status: 204});
            assert.equal(await apiRequest('/api/saved-jobs/job-id', {method: 'DELETE'}), undefined);
        });
        for (const [name, body, expected] of [
            ['controller message', {message: 'Invalid email or password.'}, 'Invalid email or password.'],
            ['validation errors', {title: 'Validation failed', errors: {Email: ['Enter an email.'], Password: ['Too short.']}}, 'Enter an email. Too short.'],
            ['problem details', {title: 'Storage error', detail: 'Storage is unavailable.'}, 'Storage is unavailable.'],
            ['JSON string', 'Skill is required.', 'Skill is required.'],
        ]) await t.test(name, async () => {
            global.fetch = async () => Response.json(body, {status: 400});
            await assert.rejects(apiRequest('/api/test'), error => error instanceof ApiError && error.status === 400 && error.message === expected);
        });
        await t.test('empty unauthorized response is actionable', async () => {
            global.fetch = async () => new Response(null, {status: 401});
            await assert.rejects(apiRequest('/api/auth/me'), /Please sign in again/);
        });
        await t.test('invalid server JSON is rejected', async () => {
            global.fetch = async () => new Response('{', {headers: {'Content-Type': 'application/json'}});
            await assert.rejects(apiRequest('/api/test'), /invalid JSON/);
        });
        await t.test('CV uploads before analysis using serialized cvid and bearer token', async () => {
            const {cvAnalysisSource} = load('api/cvs.ts', {'./client': {apiRequest}});
            const calls = [];
            global.fetch = async (url, options) => {
                calls.push(url);
                if (url === 'blob:cv') return new Response('%PDF');
                assert.equal(options.headers.get('Authorization'), 'Bearer jwt');
                if (url.endsWith('/api/cvs')) {
                    assert.equal(options.body.get('file').name, 'cv.pdf');
                    assert.equal(options.body.get('file').type, 'application/pdf');
                    return Response.json({cvid: 'cv-id'}, {status: 201});
                }
                assert.equal(options.method, 'POST');
                assert.equal(options.body, undefined);
                return Response.json({qualityScore: 75});
            };
            let result;
            await cvAnalysisSource('jwt', value => {result = value;})({document: {uri: 'blob:cv', name: 'cv.pdf'}, signal: new AbortController().signal, onProgress() {}});
            assert.deepEqual(calls, ['blob:cv', 'https://localhost:7286/api/cvs', 'https://localhost:7286/api/cvs/cv-id/analyse']);
            assert.equal(result.qualityScore, 75);
        });
        await t.test('CV analysis retries reuse the uploaded CV', async () => {
            const {cvAnalysisSource} = load('api/cvs.ts', {'./client': {apiRequest}});
            const calls = [];
            let analysisAttempts = 0;
            global.fetch = async (url, options) => {
                calls.push(url);
                if (url === 'blob:retry') return new Response('%PDF');
                if (url.endsWith('/api/cvs')) return Response.json({cvid: 'stable-cv'}, {status: 201});
                analysisAttempts++;
                if (analysisAttempts === 1) return Response.json({message: 'Analysis unavailable'}, {status: 502});
                return Response.json({qualityScore: 80});
            };
            const source = cvAnalysisSource('jwt', () => {});
            const request = {document: {uri: 'blob:retry', name: 'cv.pdf'}, signal: new AbortController().signal, onProgress() {}};
            await assert.rejects(source(request), /Analysis unavailable/);
            await source(request);
            assert.deepEqual(calls, [
                'blob:retry',
                'https://localhost:7286/api/cvs',
                'https://localhost:7286/api/cvs/stable-cv/analyse',
                'https://localhost:7286/api/cvs/stable-cv/analyse',
            ]);
        });
    } finally {
        global.fetch = originalFetch;
        if (originalUrl === undefined) delete process.env.NEXT_PUBLIC_API_BASE_URL;
        else process.env.NEXT_PUBLIC_API_BASE_URL = originalUrl;
    }
});
