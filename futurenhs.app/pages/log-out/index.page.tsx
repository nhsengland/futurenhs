import { GetServerSideProps } from 'next'

import { withLogOut } from '@hofs/withLogOut'
import { GetServerSidePropsContext } from '@appTypes/next'

const NoopTemplate = (props: any) => null

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withLogOut({
    props: {},
    getServerSideProps: async (context: GetServerSidePropsContext) => {
        return {
            redirect: {
                permanent: false,
                destination: process.env.NEXT_PUBLIC_MVC_FORUM_LOGIN_URL,
            },
        }
    },
})

/**
 * Export page template
 */
export default NoopTemplate
