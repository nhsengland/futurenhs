import Head from 'next/head';

import { Link } from '@components/Link';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

/**
 * Search template
 */
export const SearchTemplate: (props) => JSX.Element = ({
    term,
    content
}) => {

    const { metaDescriptionText, 
            titleText, 
            mainHeadingHtml } = content ?? {};

    return (

        <StandardLayout className="u-bg-theme-3">
            <Head>
                <title>{titleText}</title>
                <meta name="description" content={metaDescriptionText} />
            </Head>
            <LayoutColumnContainer>
                <LayoutColumn desktop={8}>
                    <h1>{`${mainHeadingHtml}: ${term} - 0 Results Found`}</h1>
                    <p>Sorry no results found</p>
                </LayoutColumn>
                <LayoutColumn desktop={4}>
                    <aside>
                        <h2>Groups</h2>
                        <p>
                            <Link href="/groups">View All Groups</Link>
                        </p>
                    </aside>
                </LayoutColumn>
            </LayoutColumnContainer>
        </StandardLayout>

    )

}
