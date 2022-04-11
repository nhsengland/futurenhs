import { ServiceResponse } from '@appTypes/service'
import { GenericPageTextContent } from '@appTypes/content'

declare type Options = {
    id: string
    locale?: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export type GetPageTextContentService = (
    options: Options,
    dependencies?: Dependencies
) => Promise<ServiceResponse<Record<any, any>>>

export const getPageTextContent = async (
    { id, locale }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<GenericPageTextContent>> => {
    // TODO: use locale to return appropriate content

    return new Promise((resolve: Function, reject: Function) => {
        import(`../../content-configs/${id}.ts`)
            .then(({ default: content }) => {
                resolve({
                    data: content,
                })
            })
            .catch((error) => {
                const { message } = error

                resolve({
                    data: null,
                    errors: { error: message },
                })
            })
    })
}
