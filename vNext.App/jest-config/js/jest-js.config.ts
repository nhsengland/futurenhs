const { jsWithTs: preset } = require('ts-jest/presets');
const { resolve } = require('path');
const { pathsToModuleNameMapper } = require('ts-jest/utils');
const { compilerOptions } = require('../../tsconfig');

const root = resolve(__dirname, '../../');

export default {
    displayName: 'js',
    rootDir: root,
    transform: {
        ...preset.transform,
    },
    testEnvironment: 'jsdom',
    globalSetup: '<rootDir>/jest.global.setup.ts',
    globalTeardown: '<rootDir>/jest.global.teardown.ts',
    setupFilesAfterEnv: ['./jest-config/js/jest-js-setup.ts'],
    testMatch: [
        '<rootDir>/components/**/*.test.{ts,tsx,js,jsx}', 
        '<rootDir>/pages/**/*.test.{ts,tsx,js,jsx}', 
        '<rootDir>/helpers/**/*.test.{ts,tsx,js,jsx}',
        '<rootDir>/hooks/**/*.test.{ts,tsx,js,jsx}',
        '<rootDir>/hofs/**/*.test.{ts,tsx,js,jsx}',
        '<rootDir>/selectors/**/*.test.{ts,tsx,js,jsx}',
        '<rootDir>/services/**/*.test.{ts,tsx,js,jsx}'
    ],
    reporters: [
        'default', 
        ['jest-junit', { 
            suiteName: 'Jest tests',
            outputDirectory: '<rootDir>/js-report',
            outputName: 'jest-junit.xml'
        }]
    ],
    globals: {
        'ts-jest': {
            babelConfig: 'jest.babel.config.json',
            diagnostics: true
        }
    },
    moduleFileExtensions: [
        'ts',
        'tsx',
        'js',
        'jsx',
        'json'
    ],
    moduleNameMapper: pathsToModuleNameMapper(compilerOptions.paths, {
        prefix: '<rootDir>'
    })
}