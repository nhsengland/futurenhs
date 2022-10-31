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
        maxAge: 24 * 60 * 60, // 30 days
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
export default NextAuth(authOptions)
