import { ServiceResponse } from '@appTypes/service';
import { GenericPageContent } from '@appTypes/content';

declare type Options = ({
    id: string;
    locale?: string;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getPageContent = async ({
    id,
    locale
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<GenericPageContent>> => {

    // TODO: use locale to return appropriate content

    return new Promise((resolve: Function, reject: Function) => {

        import(`../../content-configs/${id}.ts`).then(({ default: content }) => {

            resolve({
                data: content
            });

        })
        .catch((error) => {

            const { message } = error;

            resolve({
                data: null,
                errors: { error: message }
            });

        });

    });

}