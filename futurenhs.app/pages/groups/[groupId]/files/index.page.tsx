import { GetServerSideProps } from 'next'

import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { GetServerSidePropsContext } from '@appTypes/next'

const NoopTemplate = (props: any) => null
const props: Partial<any> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: async (context: GetServerSidePropsContext) => {
            return {
                redirect: {
                    permanent: false,
                    destination: props.routes.groupFoldersRoot,
                },
            }
        },
    }),
})

/**
 * Export page template
 */
export default NoopTemplate
