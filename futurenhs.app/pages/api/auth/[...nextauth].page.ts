import NextAuth from "next-auth"
import AzureADB2CProvider from "next-auth/providers/azure-ad-b2c";

export default NextAuth({
    providers: [
        AzureADB2CProvider({
            tenantId: process.env.AZURE_AD_B2C_TENANT_NAME,
            clientId: process.env.AZURE_AD_B2C_CLIENT_ID,
            clientSecret: process.env.AZURE_AD_B2C_CLIENT_SECRET,
            primaryUserFlow: process.env.AZURE_AD_B2C_PRIMARY_USER_FLOW,
            authorization: { params: { scope: "offline_access openid" } },
            idToken: true
        }),
    ],
    pages: {
        signIn: '/auth/signin',
        signOut: '/auth/signout',
        error: '500'
    },
    callbacks: {
        async signIn({ user, account, profile, email, credentials }) {
            return true
        },
        async redirect({ url, baseUrl }) {
            return baseUrl
        },
        async session({ session, user, token }) {
            session.sub = token.sub;
            return session
        },
        async jwt({ token, user, account, profile, isNewUser }) {
            return token
        }
    }
})