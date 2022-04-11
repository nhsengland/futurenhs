import { GetServerSideProps } from 'next'

import { authCookie } from '@constants/cookies'
import { cacheNames } from '@constants/caches'
import { clearClientCaches } from '@helpers/util/data'
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next'

export const withLogOut = (
    config: HofConfig,
    dependencies?: {}
): GetServerSideProps => {
    const { getServerSideProps } = config

    return async (context: GetServerSidePropsContext): Promise<any> => {
        for (var cookieName in context.req.cookies) {
            if (cookieName === authCookie) {
                ;(context.res as any).cookie(cookieName, '', {
                    ['expires']: new Date(0),
                    ['max-age']: 0,
                })
            }
        }

        await clearClientCaches([cacheNames.NEXT_DATA])

        return await getServerSideProps(context)
    }
}
