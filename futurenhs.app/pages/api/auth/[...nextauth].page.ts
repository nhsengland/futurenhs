import { NextApiRequest, NextApiResponse } from 'next'
import NextAuth, { NextAuthOptions } from 'next-auth'
import { JWT } from 'next-auth/jwt'
import AzureADB2CProvider from 'next-auth/providers/azure-ad-b2c'

const getRefreshToken = async (token: JWT) => {
    const url = `https://${process.env.AZURE_AD_B2C_TENANT_NAME}.b2clogin.com/${process.env.AZURE_AD_B2C_TENANT_NAME}.onmicrosoft.com/${process.env.AZURE_AD_B2C_PRIMARY_USER_FLOW}/oauth2/v2.0/token`
    const { refreshToken: refresh_token } = <{ refreshToken: string }>token
    const formData = {
        client_id: process.env.AZURE_AD_B2C_CLIENT_ID,
        client_secret: process.env.AZURE_AD_B2C_CLIENT_SECRET,
        scope: `${process.env.AZURE_AD_B2C_CLIENT_ID} openid offline_access`,
        grant_type: 'refresh_token',
        refresh_token,
    }
    const formBody = Object.entries(formData).map(([key, val]) => {
        const encodedKey = encodeURIComponent(key)
        const encodedValue = encodeURIComponent(val)
        return encodedKey + '=' + encodedValue
    })
    const body = formBody.join('&')
    try {
        const res = await fetch(url, {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            method: 'POST',
            body,
        })

        const refreshedTokens = await res.json()

        if (!res.ok) {
            throw refreshedTokens
        }

        return {
            ...token,
            accessToken: refreshedTokens.access_token,
            accessTokenExpires: refreshedTokens.expires_on * 1000,
            refreshToken: refreshedTokens.refresh_token ?? token.refreshToken, // Fall back to old refresh token
        }
    } catch (error) {
        return {
            ...token,
            error: 'RefreshAccessTokenError',
        }
    }
}

export const authOptions: NextAuthOptions = {
    providers: [
        AzureADB2CProvider({
            tenantId: process.env.AZURE_AD_B2C_TENANT_NAME,
            clientId: process.env.AZURE_AD_B2C_CLIENT_ID,
            clientSecret: process.env.AZURE_AD_B2C_CLIENT_SECRET,
            primaryUserFlow: process.env.AZURE_AD_B2C_PRIMARY_USER_FLOW,
            authorization: {
                params: {
                    scope: `offline_access openid ${process.env.AZURE_AD_B2C_CLIENT_ID}`,
                },
            },
            idToken: true,
        }),
    ],
    pages: {
        signIn: '/auth/signin',
        signOut: '/auth/signout',
        error: '500',
    },
    cookies: {
        sessionToken: {
            name: 'next-auth.session-token',
            options: {
                path: '/',
                httpOnly: true,
                sameSite: 'lax',
                secure: true,
            },
        },
    },
    session: {
        maxAge: 24 * 60 * 60, // 24 hours
    },
    callbacks: {
        async signIn({ user, account, profile, email, credentials }) {
            if (profile?.iss) user.iss = profile?.iss
            return true
        },
        async redirect({ url, baseUrl }) {
            return baseUrl
        },
        async session({ session, user, token }) {
            if (token?.sub) session.sub = token.sub
            if (token?.iss) session.iss = token.iss
            if (token?.id_token) session.id_token = token.id_token
            if (token?.accessToken) session.accesstoken = token.accessToken
            return session
        },
        async jwt({ token, user, account, isNewUser, profile }) {
            if (account?.id_token) token.id_token = account.id_token
            if (profile?.iss) token.iss = profile.iss
            if (account) {
                token.accessToken = account.access_token
                token.refreshToken = account.refresh_token
                token.id = profile.id
                token.accessTokenExpires = account.expires_at * 1000
            }
            if (Date.now() < token.accessTokenExpires) {
                return token
            }

            const refreshedToken = getRefreshToken(token)
            return refreshedToken
        },
    },
}

export default (req: NextApiRequest, res: NextApiResponse) => {
    const clientId = process.env.AZURE_AD_B2C_CLIENT_ID
    const tenantName = process.env.AZURE_AD_B2C_TENANT_NAME

    // #region Handle Azure B2C callbacks

    // https://docs.microsoft.com/en-us/azure/active-directory-b2c/error-codes
    const b2cPasswordResetErrorCode = 'AADB2C90118' // error_description: 'AADB2C90118: The user has forgotten their password.\r\n'
    const b2cPasswordResetCancelErrorCode = 'AADB2C90091' // error_description: 'AADB2C90091: The user has cancelled entering self-asserted information.\r\n'

    const isErrorCallback = req.url.indexOf('error=access_denied')
    if (isErrorCallback) {
        if (req.url.indexOf(b2cPasswordResetErrorCode) !== -1) {
            const b2cPasswordResetFlow =
                process.env.AZURE_AD_B2C_PASSWORD_RESET_USER_FLOW
            const redirectUri = `${process.env.APP_URL}/api/auth/signin`
            let b2cPasswordResetFlowUrl =
                `https://${tenantName}.b2clogin.com/${tenantName}.onmicrosoft.com/${b2cPasswordResetFlow}/oauth2/v2.0/authorize?
            response_type=code+id_token
            &response_mode=form_post&
            scope=offline_access%20openid
            &redirect_uri=${encodeURIComponent(redirectUri)}
            &client_id=${clientId}
            `
                    .replace(/ /g, '')
                    .replace(/\n/g, '')
                    .replace(/\r\n/g, '')
            return res.redirect(b2cPasswordResetFlowUrl)
        }
        // handle cancel clicked during password reset
        else if (req.url.indexOf(b2cPasswordResetCancelErrorCode) !== -1) {
            return res.redirect(process.env.NEXTAUTH_URL)
        }
    }

    return NextAuth(req, res, authOptions)
}
