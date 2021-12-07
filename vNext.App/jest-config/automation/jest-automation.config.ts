const tsPreset = require('ts-jest/jest-preset');
const { resolve } = require('path');
const root = resolve(__dirname, '../..');

export default {
    displayName: 'automation',
    rootDir: root,
    preset: 'jest-puppeteer',
    transform: {
        ...tsPreset.transform,
    },
    globalSetup: '<rootDir>/jest.global.setup.ts',
    globalTeardown: '<rootDir>/jest.global.teardown.ts',
    setupFilesAfterEnv: ['./jest-config/automation/jest-automation-setup.ts'],
    testEnvironment: 'node',
    testMatch: ['<rootDir>/automation-tests/**/*test.js'],
    globals: {
        'ts-jest': {
            babelConfig: 'jest.babel.config.json',
            diagnostics: true
        }
    }
}
