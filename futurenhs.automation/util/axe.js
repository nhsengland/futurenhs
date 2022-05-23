const axeSource = require('axe-core').source;
const axeReport = require('axe-html-reporter');

class axeCore{
    /**
     * Main method called by the Step Definition this method contains the steps to complete the full axe test
     * @param {*} option - Optional config setting that comes from the Step/Feature to choose what runOptions to give Axe
     * @param {*} passive - Config setting that comes from wdio.conf if truthy, the test assertions are not performed so as to not stop test run on a failure
     */
    axeTest(option, passive){
        this.injectScript();
        let axeRun = this.runAxe(option);
        let axeResults = this.evaluateExemptions(axeRun);
        if(!passive){ this.assertTestResults(axeResults) }
    }

    injectScript(){
        browser.execute(axeSource)
    }

    /**
     * Config settings for the Axe run to determine specific test standards
     * @param {*} option - key coming from the Step/Feature to match to the "standards" object
     * @returns - chosen standard for axe to test for
     */
    runOptions(option){
        // configurable options for accessibility test
        var standards = {
            all:{},
            wcag2a: {
                runOnly:{
                    types: "tag",
                    values: ['wcag2a']
                }
            },
            wcag2aa:{
                runOnly:{
                    types: "tag",
                    values: ['wcag2aa']
                }
            }
        }
        // return the desired test options for the test run
        return standards[option]
    }

    /**
     * Axe run method, to execute the test against the open browser page using the specified standards
     * Generates and Outputs the html report of the test results 
     * @param {*} option - key coming from the Step/Feature to match to the "standards" object
     * @returns - test results from the axe test
     */
    runAxe(option){
        if(!option){ option = 'all'}
        // run axe-core accesibility check against current page
        let results = browser.executeAsync(function (options, done){
            axe.run(options, function (err, results) {
                if (err) done(err)
                done(results);
            });
        }, this.runOptions(option));
        // generate html report from the accesibility
        const { pathname } = new URL(browser.getUrl());
        const fileName = pathname.replace(/\//g, '_');
        axeReport.createHtmlReport({
            results,
            options: {
                outputDir: './axeResults',
                reportFileName: `axeReport-${fileName}.html`
            }
        });
        return results
    }

    /**
     * Evaluates Incomplete/Violations from the test results and passes them through custom exemptions as needed per project 
     * Original results are cloned and emptied, and then any that don't match the exemptions are pushed back into the original results
     * @param {*} results - The axe test results original state, cloned and then emptied
     * @returns - new results values minus any results falling under the custom exemptions
     */
    evaluateExemptions(results){
        const incomplete = results.incomplete
        const violations = results.violations
        results.incomplete = [];
        results.violations = [];
        incomplete.forEach((issue) => {
            if(!this.shouldIgnoreIssue(issue)){
                results.incomplete.push(issue);
            }
        });
        violations.forEach((issue) => {
            if(!this.shouldIgnoreIssue(issue)){
                results.violations.push(issue);
            }
        });
        return results
    }

    /**
     * Test assertions against the Incomplete and Violations results
     * @param {*} results - New results file after being passed through the evaluateExemptions method
     */
    assertTestResults(results){
        expect(results.incomplete.length).toEqual(0);
        expect(results.violations.length).toEqual(0);
    }

    /**
     * Custom method to find any issue that matches the condition and ignoring said issue
     * @param {*} issue - Individual result found within the Axe Results
     * @returns - true/false
     */
    shouldIgnoreIssue(issue) {
        let shouldIgnore = false;
        console.log(issue)
        // aXe returns a false positive on colour contrast for elements with a pseudo-element which has a background colour
        if (issue.id === 'color-contrast' ||
        // Known issue with govuk autocomplete component, aria value is false until an option is selected
            (issue.id === 'aria-valid-attr-value' && issue.nodes[0]?.failureSummary?.includes('aria-activedescendant="false"'))
        ) {
            shouldIgnore = true;
        }
        return shouldIgnore;
    }
}
module.exports = new axeCore();