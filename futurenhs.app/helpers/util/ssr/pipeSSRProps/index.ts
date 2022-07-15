import { GetServerSidePropsResult, GetServerSidePropsContext } from 'next'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { Hof, HofConfig } from '@appTypes/hof'

/**
 * Creates a page object in the server-side context object,
 * pipes the context object through a series of higher order functions to hydrate with data
 * then returns the hydrated context object to a provided callback handler
 */
export const pipeSSRProps = async (
    context,
    page,
    hofs: Array<Hof | [Hof, HofConfig]> = [],
    callBack: (GetServerSidePropsContext: GetServerSidePropsContext) => Promise<GetServerSidePropsResult<any>>
) => {

    let result: GetServerSidePropsResult<any> = null;

    /**
     * Create a page object
     */
    context.page = page;
    context.page.props = context.page.props ?? {};

    /**
     * Iterate through provided higher order functions
     */
    for (let i = 0; i < hofs.length; i++) {

        const hof = hofs[i];

        try {

            /**
             * Higher order functions can be passed as a function reference
             * or a tuple containing the function reference and an associated config object
             */
            const hofFunc: Hof = Array.isArray(hof) ? hof[0] : hof;
            const config = Array.isArray(hof) ? hof[1] : {};

            const hofResult = await hofFunc(context, config)

            /**
             * Higher order functions may return a next getServerSidePropsResult object e.g. to signify a redirect or not found,
             * at which point the pipe should be terminated 
             */
            if (hofResult) {

                result = hofResult;
                break;

            }

        } catch (error) {

            result = handleSSRErrorProps({ props: context.page.props, error });
            break;

        }

    }

    /**
     * Return either the getServerSidePropsResult object if the pipe was terminated early by a higher order function,
     * or return the provided callback, passed the hydrated context
     */
    return result ? result : callBack(context);

}
