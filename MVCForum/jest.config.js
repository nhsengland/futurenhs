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
        './MVCForum/setupTests.js'
    ],
    reporters: [
        'default', 
        ['jest-junit', { 
            suiteName: 'Jest tests',
            outputDirectory: './MVCForum/js-report',
            outputName: 'jest-junit.xml'
        }]
    ],
    collectCoverage: true,
    collectCoverageFrom: [
        'MVCForum/MVCForum.Website/UI/assets/src/ts/**/*.{ts,tsx,js,jsx}'
    ],
    coverageDirectory: 'MVCForum/js-report',
    coveragePathIgnorePatterns: [
        './node_modules/',
        'MVCForum/MVCForum.Website/UI/assets/src/ts/modules/utilities/index',
        'MVCForum/MVCForum.Website/UI/assets/src/ts/modules/ui/index',
        'MVCForum/MVCForum.Website/UI/assets/src/ts/modules/polyfills/*',
        'MVCForum/MVCForum.Website/UI/assets/src/ts/root/global',
        'MVCForum/MVCForum.Website/UI/assets/src/ts/root/polyfills',
        'MVCForum/MVCForum.Website/UI/assets/src/ts/types/*',
        'futurenhs.app/*'
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
        prefix: 'MVCForum/MVCForum.Website/UI/assets/src'
    })
};