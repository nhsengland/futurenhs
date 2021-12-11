import Head from 'next/head';
import { GetServerSideProps } from 'next';
import Link from 'next/link';

import { Layout } from '@components/Layout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { withLogOut } from '@hofs/withLogOut';
import { getEnvVar } from '@helpers/util/env';
import { GetServerSidePropsContext } from '@appTypes/next';

import { Props } from './interfaces';

const Index: (props: Props) => JSX.Element = ({
    content,
    logOutUrl
}) => {

    const { titleText, 
            metaDescriptionText, 
            mainHeadingHtml } = content ?? {};

    return (

        <Layout 
            shouldRenderSearch={false}
            shouldRenderUserNavigation={false}
            shouldRenderMainNav={false}>
                <Head>
                    <title>{titleText}</title>
                    <meta name="description" content={metaDescriptionText} />
                </Head>
                <LayoutColumnContainer justify="centre">
                    <LayoutColumn tablet={6} className="u-py-10">
                        <h1>{mainHeadingHtml}</h1>
                        <p>Your are now logged out</p>
                        <p className="desktop:u-pb-4">
                            <Link href={logOutUrl}>
                                <a>Log in again</a>
                            </Link>
                        </p>
                    </LayoutColumn>
                </LayoutColumnContainer>
        </Layout>

    )

}

export const getServerSideProps: GetServerSideProps = withLogOut(async(context: GetServerSidePropsContext) => {

    return {
        props: {
            content: {
                titleText: 'Logged out', 
                metaDescriptionText: 'Log out',
                mainHeadingHtml: 'Logged out'
            },
            logOutUrl: getEnvVar({ name: 'NEXT_PUBLIC_MVC_FORUM_LOGIN_URL' })
        }
    }

});

export default Index;