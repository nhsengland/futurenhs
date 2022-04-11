import { getEnvVar } from '../util/env'

export const setUpPage = async ({ browser, url }): Promise<any> => {
    const page = await browser.newPage()
    const client = await page.target().createCDPSession()

    await client.send('Network.clearBrowserCookies')
    await page.setDefaultNavigationTimeout(100000)
    await page.goto(url)
    await page.setViewport({
        width: 1920,
        height: 1080,
    })

    return page
}

export const logIn = async ({
    page,
}): Promise<{
    cookies: Array<Record<string, any>>
}> => {
    const userNameField: string = '[name=UserName]'
    const passWordField: string = '[name=Password]'
    const submitButton: string = '[type="submit"]'

    let cookies: Array<Record<string, any>>

    await page.waitForSelector(userNameField)
    await page.click(userNameField)
    await page.type(userNameField, getEnvVar({ name: 'LOGIN_USER_NAME' }))
    await page.waitForSelector(passWordField)
    await page.click(passWordField)
    await page.type(passWordField, getEnvVar({ name: 'LOGIN_PASSWORD' }))
    await page.waitForSelector(submitButton)
    await page.click(submitButton)
    await page.waitForNavigation()

    cookies = await page.cookies()

    return {
        cookies: cookies,
    }
}
