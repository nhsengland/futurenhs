import Head from 'next/head';
import Link from 'next/link';
import { GetServerSideProps } from 'next';

import { Layout } from '@components/Layout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { PageHeader } from '@components/PageHeader';
import { Card } from '@components/Card';
import { SVGIcon } from '@components/SVGIcon';
import { PaginatedList } from '@components/PaginatedList';
import { withAuth } from '@hofs/withAuth';
import { getGroups } from '@services/getGroups';
import { selectUser, selectPagination } from '@selectors/context';
import { ServicePaginatedResponse } from '@appTypes/service';
import { GetServerSidePropsContext } from '@appTypes/next';
import { Group } from '@appTypes/group';
import { User } from '@appTypes/user';

import { Props } from './interfaces';

const Index: (props: Props) => JSX.Element = ({
    user,
    content,
    groupsList
}) => {

    const { titleText, 
            metaDescriptionText, 
            mainHeadingHtml } = content ?? {};

    return (

        <Layout user={user}>
            <Head>
                <title>{titleText}</title>
                <meta name="description" content={metaDescriptionText} />
            </Head>
            <LayoutColumnContainer>
                <PageHeader
                    id="discover-groups"
                    content={{
                        mainHeadingHtml: mainHeadingHtml, 
                        descriptionHtml: 'Share ideas, get advice and support from your peers',
                        navMenuTitleText: 'Group menu'
                    }} 
                    navMenuList={[
                        {
                            url: '/groups',
                            text: 'My groups'
                        },
                        {
                            url: '/groups/discover',
                            text: 'Discover new groups'
                        }
                    ]}
                    className="u-bg-theme-14" />
                <LayoutColumn className="u-px-4 u-py-10">
                    <h2>Discover Groups</h2>
                    <PaginatedList {...groupsList}>
                        {groupsList?.data?.map(({ 
                            image, 
                            content, 
                            slug, 
                            totalDiscussionCount, 
                            totalMemberCount 
                        }, index) => {

                            const { mainHeadingHtml, strapLineText } = content ?? {};

                            return (

                                <Card key={index} image={image} className="u-border-bottom-theme-8">
                                    <h3 className="c-card_heading">
                                        <Link href={`/groups/${slug}`}>
                                            <a>{mainHeadingHtml}</a>
                                        </Link>        
                                    </h3>
                                    <p className="c-card_content u-text-theme-7 o-truncated-text-lines-2">
                                        {strapLineText}
                                    </p>
                                    <div className="c-card_footer u-text-theme-7">
                                        <p className="c-card_footer-item">
                                            <SVGIcon name="icon-account" className="c-card_footer-icon u-fill-theme-8" />
                                            <span>{`${totalMemberCount} Members`}</span>
                                        </p>
                                        <p className="c-card_footer-item">
                                            <SVGIcon name="icon-discussion" className="c-card_footer-icon u-fill-theme-8" />
                                            <span>{`${totalDiscussionCount} Discussions`}</span>
                                        </p>
                                    </div>
                                </Card>

                            )

                        })}
                    </PaginatedList>
                </LayoutColumn>
            </LayoutColumnContainer>
        </Layout>

    )

}

export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const user: User = selectUser(context);
    const { pageNumber, pageSize } = selectPagination(context);

    const groupsList: ServicePaginatedResponse<Array<Group>> = await getGroups({
        user: user,
        filters: {
            isMember: false
        },
        pageNumber: pageNumber,
        pageSize: pageSize
    });

    return {
        props: {
            user: user,
            content: {
                titleText: 'Discover Future NHS groups', 
                metaDescriptionText: 'Discover Future NHS groups',
                mainHeadingHtml: 'Discover groups'
            },
            groupsList: groupsList
        }
    }

});

export default Index;