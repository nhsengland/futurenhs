import Head from 'next/head';
import { GetServerSideProps } from 'next';
import { useRouter } from 'next/router';

import { Layout } from '@components/Layout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { PageHeader } from '@components/PageHeader';
import { withAuth } from '@hofs/withAuth';
import { getGroup } from '@services/getGroup';
import { selectUser } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

const Index: (props) => JSX.Element = ({
    user,
    content
}) => {

    const router = useRouter();
    const currentPathName: string = router.asPath;

    const { titleText, 
            metaDescriptionText, 
            mainHeadingHtml,
            strapLineText } = content ?? {};

    return (

        <Layout user={user}>
            <Head>
                <title>{titleText}</title>
                <meta name="description" content={metaDescriptionText} />
            </Head>
            <LayoutColumnContainer>
                <PageHeader 
                    id="files"
                    content={{
                        mainHeadingHtml: mainHeadingHtml, 
                        descriptionHtml: strapLineText,
                        navMenuTitleText: 'Group menu'
                    }} 
                    navMenuList={[
                        {
                            url: `${currentPathName.replace('/files', '/')}`,
                            text: 'Home'
                        },
                        {
                            url: `${currentPathName.replace('/files', '/forum')}`,
                            text: 'Forum'
                        },
                        {
                            url: `${currentPathName}`,
                            text: 'Files'
                        },
                        {
                            url: `${currentPathName.replace('/files', '/members')}`,
                            text: 'Members'
                        }
                    ]}
                    className="u-bg-theme-14" />
                <LayoutColumn tablet={8} className="u-px-4 u-py-10">
                </LayoutColumn>
            </LayoutColumnContainer>
        </Layout>

    )

}

export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const pathElements: Array<string> = context.resolvedUrl.split('/');
    const slug: string = pathElements[pathElements.length - 2];
    const user: User = selectUser(context);
    const { data } = await getGroup({
        user: user,
        slug: slug,
        page: 'files'
    });

    return {
        props: {
            user: user,
            content: data.content
        }
    }

});

export default Index;