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
    content,
    image
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
                    id="group"
                    image={image}
                    content={{
                        mainHeadingHtml: mainHeadingHtml, 
                        descriptionHtml: strapLineText,
                        navMenuTitleText: 'Group menu'
                    }} 
                    navMenuList={[
                        {
                            url: `${currentPathName}`,
                            text: 'Home'
                        },
                        {
                            url: `${currentPathName}/forum`,
                            text: 'Forum'
                        },
                        {
                            url: `${currentPathName}/files`,
                            text: 'Files'
                        },
                        {
                            url: `${currentPathName}/members`,
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

    const slug: string = context.resolvedUrl.substring(context.resolvedUrl.lastIndexOf('/') + 1);
    const user: User = selectUser(context);
    const { data, errors } = await getGroup({
        user: user,
        slug: slug,
        page: 'home'
    });

    console.log(slug, user);

    return {
        props: {
            user: user,
            content: data?.content ?? null,
            image: data?.image ?? null
        }
    }

});

export default Index;