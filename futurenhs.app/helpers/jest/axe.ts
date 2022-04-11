const { AxePuppeteer } = require('@axe-core/puppeteer')
const { createHtmlReport } = require('axe-html-reporter')
const fs = require('fs')
const axeConfig = require('../../axe.custom.config')

export const axeAudit = async ({ page }) => {
    await page.setBypassCSP(true)

    const results = await new AxePuppeteer(page)
        .exclude('.l-width-container, .c-page-header')
        .analyze()

    return new Promise((resolve, reject) => {
        const { name, outputPath } = axeConfig.report

        if (outputPath) {
            const { pathname } = new URL(page.url())

            let fileNameBase: string = pathname.replace(/\//g, '_')

            if (fileNameBase === '_') {
                fileNameBase = 'index'
            }

            const fileName: string = `${fileNameBase}.html`

            fs.mkdir(
                outputPath,
                {
                    recursive: true,
                },
                (error) => {
                    if (error) {
                        reject(error)
                    }

                    createHtmlReport({
                        results: results,
                        options: {
                            projectKey: name,
                            outputDir: outputPath,
                            reportFileName: fileName,
                        },
                    })
                }
            )
        }

        const incomplete: Array<any> = [...results.incomplete]
        const violations: Array<any> = [...results.violations]

        results.incomplete = []
        results.violations = []

        incomplete.forEach((issue) => {
            if (!axeConfig.shouldIgnoreIssue(issue)) {
                results.incomplete.push(issue)
            }
        })

        violations.forEach((issue) => {
            if (!axeConfig.shouldIgnoreIssue(issue)) {
                results.violations.push(issue)
            }
        })

        resolve(results)
    })
}
