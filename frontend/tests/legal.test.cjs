const {test} = require('node:test');
const assert = require('node:assert/strict');
const load = require('./load-module.cjs');

test('legal documents publish dated, substantive terms and privacy copy', () => {
    const {legalDocuments} = load('legal/documents.ts');

    for (const document of Object.values(legalDocuments)) {
        assert.equal(document.effectiveDate, '23 October 2026');
        assert.ok(document.lead.length > 80);
        assert.ok(document.sections.length >= 10);
        assert.ok(document.sections.every(section => section.heading.length > 0 && section.body.length > 80));
    }
    assert.match(legalDocuments.privacy.sections.map(section => section.body).join(' '), /n8n workflows/);
    assert.match(legalDocuments.privacy.sections.map(section => section.body).join(' '), /POPIAComplaints@inforegulator\.org\.za/);
    assert.match(legalDocuments.terms.sections.map(section => section.body).join(' '), /AI-generated guidance/);
});

test('consent from an older legal-document version is not reused', () => {
    const values = new Map();
    global.localStorage = {
        getItem:key => values.get(key) ?? null,
        setItem:(key,value) => values.set(key,value),
    };
    const privacyTypes = load('privacy/types.ts');
    const {loadConsent, saveConsent} = load('privacy/storage.ts', {'./types':privacyTypes});
    const owner = 'person@example.com';

    values.set('career-path-consent-person@example.com', JSON.stringify({
        status:'granted', updatedAt:'2026-01-01T00:00:00.000Z', documentVersion:'draft',
    }));
    assert.equal(loadConsent(owner), null);

    const current = {status:'granted', updatedAt:'2026-10-23T00:00:00.000Z', documentVersion:privacyTypes.documentVersion};
    assert.equal(saveConsent(owner, current), true);
    assert.deepEqual(loadConsent(owner), current);
    delete global.localStorage;
});
