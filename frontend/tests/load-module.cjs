const fs = require('node:fs');
const path = require('node:path');
const ts = require('typescript');
module.exports = function load(relative, dependencies = {}) {
    const source = fs.readFileSync(path.join(__dirname, '../src', relative), 'utf8');
    const compiled = ts.transpileModule(source, {fileName: relative, compilerOptions: {jsx: ts.JsxEmit.ReactJSX, module: ts.ModuleKind.CommonJS, target: ts.ScriptTarget.ES2022}}).outputText;
    const module = {exports: {}};
    new Function('require', 'module', 'exports', compiled)(name => dependencies[name] || require(name), module, module.exports);
    return module.exports;
};
