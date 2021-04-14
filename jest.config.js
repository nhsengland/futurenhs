const { pathsToModuleNameMapper } = require('ts-jest/utils');
const { compilerOptions } = require('./tsconfig');

module.exports = {
    verbose: true,
    bail: true,
    clearMocks: true,
    preset: 'ts-jest/presets/js-with-ts',
    testEnvironment: 'jest-environment-jsdom-fifteen',
    testPathIgnorePatterns: [
        '<rootDir>/yw.website/UI/assets/src/apps/*' // temporarily excluded while WIP
    ],
    setupFilesAfterEnv: [
        './setupTests.js'
    ],
    collectCoverage: true,
    collectCoverageFrom: [
        'yw.website/UI/assets/src/ts/**/*.{ts,tsx,js,jsx}'
    ],
    coverageDirectory: 'js-report',
    coveragePathIgnorePatterns: [
        './node_modules/',
        'yw.website/UI/assets/src/ts/modules/utilities/index',
        'yw.website/UI/assets/src/ts/modules/ui/index',
        'yw.website/UI/assets/src/ts/root/global',
        'yw.website/UI/assets/src/ts/types/*',
        'yw.website/UI/assets/src/apps/maps/*' // temporarily excluded while WIP
    ],
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
        prefix: '<rootDir>/yw.website/UI/assets/src'
    })
};