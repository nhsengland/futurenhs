import { selectLocale } from '@helpers/selectors/context'
import { getPageTextContent } from '@services/getPageTextContent'
import { GetPageTextContentService } from '@services/getPageTextContent'
import { Hof } from '@appTypes/hof'

export const withTextContent: Hof = async (
    context,
    config,
    dependencies?: {
        getPageTextContentService?: GetPageTextContentService
    }
) => {
    const getPageTextContentService =
        dependencies?.getPageTextContentService ?? getPageTextContent

    const { routeId } = context.page
    const locale: string = selectLocale(context)

    /**
     * Get data from services
     */
    try {
        const [pageTextContentData] = await Promise.all([
            getPageTextContentService({ id: routeId, locale }),
        ])

        context.page.props.contentText = pageTextContentData.data
    } catch (error) {
        console.log(error)
    }
}
