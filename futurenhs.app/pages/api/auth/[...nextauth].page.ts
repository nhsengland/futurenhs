import NextAuth, { NextAuthOptions } from 'next-auth'
import { getToken } from 'next-auth/jwt'
import AzureADB2CProvider from 'next-auth/providers/azure-ad-b2c'

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
            name: 'next-auth.session-token.1.1',
            options: {
                path: '/',
                httpOnly: true,
                sameSite: 'lax',
                secure: true,
            },
        },
    },
    callbacks: {
        async signIn({ user, account, profile, email, credentials }) {
            if (profile?.iss) console.log('issuer :' + profile?.iss)
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
                token.id = profile.id
            }
            return token
        },
    },
}
export default NextAuth(authOptions)
