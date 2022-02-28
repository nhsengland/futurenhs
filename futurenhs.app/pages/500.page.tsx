import Head from 'next/head';
import { useRouter } from 'next/router';

import { getBreadCrumbList } from '@helpers/routing/getBreadCrumb';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { BreadCrumbList } from '@appTypes/routing';

/**
 * Error 500 page
 */
const Index: (props) => JSX.Element = ({

}) => {

    const { asPath } = useRouter();

    const currentRoutePathElements: Array<string> = asPath?.split('/')?.filter((item) => item) ?? [];
    const breadCrumbList: BreadCrumbList = getBreadCrumbList({ 
        pathElementList: currentRoutePathElements, 
        shouldIncludeAllParams: true
    });

    return (

        <StandardLayout
            shouldRenderPhaseBanner={false}
            shouldRenderMainNav={false}
            shouldRenderSearch={false}
            shouldRenderUserNavigation={false}
            breadCrumbList={breadCrumbList}
            className="u-bg-theme-1">
                <Head>
                    <title>Unexpected error</title>
                    <meta name="description" content="500: Server error" />
                </Head>
                <LayoutColumnContainer>
                    <LayoutColumn tablet={8} className="u-py-10">
                        <h1>The FutureNHS website is currently experiencing technical difficulties.</h1>
                        <p className="u-text-lead">We are working to resolve these issues. Please try again later. Thank you for your patience.</p> 
                        <p className="u-text-lead">Thank you for your patience.</p>
                    </LayoutColumn>
                </LayoutColumnContainer>
        </StandardLayout>

    )

}

export default Index;