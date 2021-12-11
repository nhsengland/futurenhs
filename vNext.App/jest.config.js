module.exports = {
	projects: [
		'<rootDir>/jest-config/automation/jest-automation.config.ts',
		'<rootDir>/jest-config/js/jest-js.config.ts'
	],
    collectCoverage: true,
    collectCoverageFrom: [
        '<rootDir>/pages/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/components/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/helpers/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/hooks/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/hofs/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/services/**/*.{ts,tsx,js,jsx}',
        '<rootDir>/selectors/**/*.{ts,tsx,js,jsx}',
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