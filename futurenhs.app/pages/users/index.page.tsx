import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { GetServerSidePropsContext } from '@appTypes/next'

const NullPage = () => null

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {},
        [withUser, withRoutes],
        async (context: GetServerSidePropsContext) => {
            return {
                redirect: {
                    permanent: false,
                    destination: '/',
                },
            }
        }
    )

/**
 * Export page template
 */
export default NullPage
