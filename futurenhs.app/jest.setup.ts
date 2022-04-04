import Adapter from 'enzyme-adapter-react-16';
import React from 'react';
import MatchMediaMock from 'jest-matchmedia-mock';
import '@testing-library/jest-dom/extend-expect';
import '@testing-library/jest-dom';

require('idempotent-babel-polyfill');
require('raf');

//DO NOT REMOVE fetch - required for testing
const fetch = require('jest-fetch-mock');
const Enzyme = require('enzyme');

let matchMedia;

global.matchMedia = (query: string): any => { };
global.scrollTo = () => { };
global.React = React;
global.console = {
    ...console,
    log: jest.fn(),
    debug: jest.fn(),
    info: jest.fn(),
    warn: jest.fn(),
    error: jest.fn(),
};

Enzyme.configure({
    adapter: new Adapter()
});

jest.setTimeout(60000);

beforeAll(() => {

    matchMedia = new MatchMediaMock();

});

afterEach(() => {

    matchMedia.clear();

});

