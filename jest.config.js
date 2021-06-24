const { pathsToModuleNameMapper } = require('ts-jest/utils');
const { compilerOptions } = require('./tsconfig');

module.exports = {
    verbose: true,
    bail: true,
    clearMocks: true,
    preset: 'ts-jest/presets/js-with-ts',
    testEnvironment: 'jsdom',
    testPathIgnorePatterns: [],
    setupFilesAfterEnv: [
        './setupTests.js'
    ],
    reporters: [
        'default', 
        ['jest-junit', { 
            suiteName: 'Jest tests',
            outputDirectory: './js-report',
            outputName: 'jest-junit.xml'
        }]
    ],
    collectCoverage: true,
    collectCoverageFrom: [
        'MVCForum.Website/UI/assets/src/ts/**/*.{ts,tsx,js,jsx}'
    ],
    coverageDirectory: 'js-report',
    coveragePathIgnorePatterns: [
        './node_modules/',
        'MVCForum.Website/UI/assets/src/ts/modules/utilities/index',
        'MVCForum.Website/UI/assets/src/ts/modules/ui/index',
        'MVCForum.Website/UI/assets/src/ts/modules/polyfills/svg',
        'MVCForum.Website/UI/assets/src/ts/root/global',
        'MVCForum.Website/UI/assets/src/ts/root/polyfills',
        'MVCForum.Website/UI/assets/src/ts/types/*',
    ],
    coverageReporters: ['json', 'lcov', 'text', 'clover', 'cobertura'],
    globals: {
        'ts-jest': {
            babelConfig: false,
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
        prefix: '<rootDir>/MVCForum.Website/UI/assets/src'
    })
};