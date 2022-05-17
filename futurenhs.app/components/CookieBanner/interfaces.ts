export interface Props {
    /**
     * Override default cookie that is stored in the browser
     */
    cookieName?: string
    /**
     * Override default value of cookie upon accepting cookies
     */
    cookieAcceptValue?: string
    /**
     * Override default value of cookie upon rejecting cookies
     */
    cookieRejectValue?: string
    /**
     * Override default length cookie is stored in the browser
     */
    expiresInDays?: number
    /**
     * Override default cookie banner text content
     */
    text?: any
}
