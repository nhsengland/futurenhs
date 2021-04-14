require('idempotent-babel-polyfill');
require('raf');

const $ = require('jquery');
const fetch = require('jest-fetch-mock');
const Enzyme = require('enzyme');

global.$ = $;
global.fetch = fetch;
global.matchMedia = () => {};
global.scrollTo = () => {};

Enzyme.configure({});

jest.setTimeout(60000);
jest.mock('@scss/variables/_break-points.scss', () => ({
    locals: {}
}));