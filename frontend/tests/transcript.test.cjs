const {test} = require('node:test');
const assert = require('node:assert/strict');
const load = require('./load-module.cjs');
test('transcripts use their own multipart endpoint and preserve CV isolation', async () => {
    const oldFetch = global.fetch;
    let received;
    try {
        global.fetch = async () => new Response('%PDF-test');
        const expected = {summary: 'Read transcript', subjects: [{subject: 'Maths', mark: 82}], detectedSkills: []};
        const {transcriptAnalysisSource} = load('api/transcripts.ts', {'./client': {apiRequest: async (path, request) => {
            assert.equal(path, '/api/transcripts/analyse');
            assert.equal(request.method, 'POST');
            assert.equal(request.token, 'jwt');
            assert.equal(request.body.get('file').name, 'academic.pdf');
            assert.equal(request.body.get('file').type, 'application/pdf');
            return expected;
        }}});
        await transcriptAnalysisSource('jwt', result => {received = result;})({document: {uri: 'blob:transcript', name: 'academic.pdf'}, signal: new AbortController().signal});
        assert.deepEqual(received, expected);
    } finally {global.fetch = oldFetch;}
});
