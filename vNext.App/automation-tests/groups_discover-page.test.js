const puppeteer = require('puppeteer');
const { setUpPage, logIn } = require('../helpers/jest/puppeteer');
const { axeAudit } = require('../helpers/jest/aXe');
const { lighthouseAudit, getLighthouseResult } = require('../helpers/jest/lighthouse');
const { getEnvVar } = require('../helpers/util/env');

describe('Groups page', () => {

    const url = `${getEnvVar({ name: 'APP_URL' })}/groups/discover`;

    let browser;

    beforeEach(async () => {

        browser = await puppeteer.launch();

    });

    afterEach(async () => {

        await browser.close();

    });

    test('Is login protected', async () => {

        const page = await setUpPage({ browser, url });
        
        await expect(page.url()).toEqual(getEnvVar({ name: 'NEXT_PUBLIC_MVC_FORUM_LOGIN_URL'}));
        await logIn({
            page: page
        });
        await expect(page.url()).toEqual(getEnvVar({ name: 'APP_URL'}) + '/');

    });

    test('Is valid and accessible', async () => {

        const page = await setUpPage({ browser, url });

        await logIn({
            page: page
        });
        await page.goto(url);

        const { incomplete, violations } = await axeAudit({ page });

        expect(incomplete).toHaveLength(0);
        expect(violations).toHaveLength(0);
        
    });

    test('Is performant and follows best practices', async () => {

        const page = await setUpPage({ browser, url });
        const { cookies } = await logIn({
            page: page
        });
        await page.close();

        const lhr = await lighthouseAudit({ browser, url, cookies });
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
