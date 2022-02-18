const { jsWithTs: preset } = require('ts-jest/presets');
const { pathsToModuleNameMapper } = require('ts-jest/utils');
const { compilerOptions } = require('./tsconfig');
const { loadEnvConfig } = require('@next/env');
const projectDir = process.cwd();

loadEnvConfig(projectDir);

module.exports = {
    transform: {
        ...preset.transform,
    },
    testEnvironment: 'jsdom',
    setupFilesAfterEnv: ['./jest.setup.ts'],
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
    }),
    collectCoverage: true,
    collectCoverageFrom: [
        '<rootDir>/components/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/helpers/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/hooks/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/hofs/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/services/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/selectors/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/pages/**/*.{ts,tsx,js,jsx}',
    ],
    coverageDirectory: 'test-reports/unit',
    coveragePathIgnorePatterns: [
        '<rootDir>/node_modules/',
        '<rootDir>/UI/'
    ],
    coverageReporters: ['json', 'lcov', 'text', 'clover', 'cobertura'],
    reporters: [
        'default', 
        ['jest-junit', { 
            suiteName: 'Jest tests',
            outputDirectory: './test-reports/unit',
            outputName: 'jest-junit.xml'
        }]
    ],
}