const puppeteer = require('puppeteer');
const { setUpPage } = require('../helpers/jest/puppeteer');
const { axeAudit } = require('../helpers/jest/aXe');
const { lighthouseAudit, getLighthouseResult } = require('../helpers/jest/lighthouse');

describe('Cookies page', () => {

    const url = `${process.env.APP_URL}/cookies`;

    let browser;

    beforeEach(async () => {

        browser = await puppeteer.launch();

    });

    afterEach(async () => {

        await browser.close();

    });

    test('Is valid and accessible', async () => {

        const page = await setUpPage({ browser, url });

        const { incomplete, violations } = await axeAudit({ page });

        expect(incomplete).toHaveLength(0);
        expect(violations).toHaveLength(0);

    });

    test('Is performant and follows best practices', async () => {

        const lhr = await lighthouseAudit({ browser, url });
        const performanceScore = await getLighthouseResult({ lhr, property: 'performance' });
        const pageSpeedScore = await getLighthouseResult({ lhr, property: 'pageSpeed' });
        const bestPracticesScore = await getLighthouseResult({ lhr, property: 'bestPractices' });
        const seoScore = await getLighthouseResult({ lhr, property: 'seo' });

        expect(performanceScore).toBeGreaterThanOrEqual(80);
        expect(pageSpeedScore).toBeGreaterThanOrEqual(90);
        expect(bestPracticesScore).toBeGreaterThanOrEqual(90);
        expect(seoScore).toBeGreaterThanOrEqual(100);   
        
    });

});
