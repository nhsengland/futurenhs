const lighthouseSource = require('lighthouse');
const fs = require('fs');
const chromeLauncher = require('chrome-launcher');
const windowSize = [1920, 1080];

class lighthouse{

    /**
     * Main method called by the Step Definition that handles all the steps in running the lighthouse test
     * @param {*} desktop - truthy value to switch Lighthouse to run with desktop config instead of default mobile
     */
    async runLighthouse(desktop){
        const url = await browser.getUrl();
        const lhr = await this.lighthouseAudit(url, desktop);
        // Get results from the lighthouse report
        const performanceScore = await this.getLighthouseResult({ lhr, property: 'performance' });
        const pageSpeedScore = await this.getLighthouseResult({ lhr, property: 'pageSpeed' });
        const bestPracticesScore = await this.getLighthouseResult({ lhr, property: 'bestPractices' });
        const seoScore = await this.getLighthouseResult({ lhr, property: 'seo' });
        // Test assertions on the report scores
        expect(performanceScore).toBeGreaterThanOrEqual(80);
        expect(pageSpeedScore).toBeGreaterThanOrEqual(90);
        expect(bestPracticesScore).toBeGreaterThanOrEqual(90);
        expect(seoScore).toBeGreaterThanOrEqual(100);
    }

    /**
     * Main test execution method, that runs the lighthouse test and outputs the html result
     * @param {*} url - Current url for the test to be executed against
     * @param {*} desktop - truthy value to switch Lighthouse to run with desktop config instead of default mobile
     * @returns - Test results of the Lighthouse test run
     */
    async lighthouseAudit(url, desktop){   
        const chrome = await chromeLauncher.launch({chromeFlags:['--headless', `--set-window-size=${windowSize[0]},${windowSize[1]}`]});
        const cookies = await browser.getCookies();
        // run lighthouse test and get report
        const lighthouseReport = await lighthouseSource(url,
        {
            port: chrome.port,
            logLevel: 'error',
            output: 'html',
            disableStorageReset: false,
            extraHeaders: {
                Cookie: cookies.map(cookie => `${cookie.name}=${cookie.value}; `).join("")
            },
        },
            desktop ? {
                // Config settings to match the Lighthouse Chrome Extension settings:
                extends: 'lighthouse:default',
                settings: {
                    formFactor: 'desktop',
                    throttling: {
                        rttMs: 40,
                        throughputKbps: 10240,
                        cpuSlowdownMultiplier: 1,
                        requestLatencyMs: 0,
                        downloadThroughputKbps: 0,
                        uploadThroughputKbps: 0
                    },
                    screenEmulation: {
                        mobile: false,
                        width: windowSize[0],
                        height: windowSize[1],
                        deviceScaleFactor: 1,
                        disabled: false
                    },
                emulatedUserAgent: 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4143.7 Safari/537.36 Chrome-Lighthouse'
            }
        } : { 
            extends: 'lighthouse:default' 
        });
        //generate html report from the lighthouse run
        const {pathname} = new URL(await browser.getUrl());
        const fileName = pathname.replace(/\//g, '_');
        if(!fs.existsSync('./lighthouseReports')) {fs.mkdirSync('./lighthouseReports')};
        fs.writeFileSync(`./lighthouseReports/${fileName}.html`, lighthouseReport.report);
        await chrome.kill();
        return lighthouseReport;
    };

    /**
     * Method to convert lighthouse results into % value for simple assertions
     * @param {*} lhr - Test results of the Lighthouse test run
     * @returns - New results object with percentage values to be asserted against
     */
    async getLighthouseResult({ lhr, property }){
        const jsonProperty = new Map()
            .set('accessibility', await lhr.lhr.categories.accessibility.score * 100)
            .set('performance', await lhr.lhr.categories.performance.score * 100)
            .set('progressiveWebApp', await lhr.lhr.categories.pwa.score * 100)
            .set('bestPractices', await lhr.lhr.categories["best-practices"].score * 100)
            .set('seo', await lhr.lhr.categories.seo.score * 100)
            .set('pageSpeed', await lhr.lhr.audits["speed-index"].score * 100);
        let result = await jsonProperty.get(property);
        return result;
    }
}
module.exports = new lighthouse();