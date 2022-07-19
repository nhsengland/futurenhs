import { authCookie } from '@constants/cookies'
import { cacheNames } from '@constants/caches'
import { clearClientCaches } from '@helpers/util/data'
import { Hof } from '@appTypes/hof'

export const withLogOut: Hof = async (context) => {

    for (var cookieName in context.req.cookies) {
        if (cookieName === authCookie) {
            ; (context.res as any).cookie(cookieName, '', {
                ['expires']: new Date(0),
                ['max-age']: 0,
            })
        }
    }

    await clearClientCaches([cacheNames.NEXT_DATA])

}
