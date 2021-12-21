import classNames from 'classnames';
import Head from 'next/head';
import { useRouter } from 'next/router';

import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { PageHeader } from '@components/PageHeader';
import { getGroupNavMenuList } from '@helpers/routing/getGroupNavMenuList';

import { Props } from './interfaces';

export const GroupLayout: (props: Props) => JSX.Element = ({
    id,
    content,
    image,
    children,
    ...rest 
}) => {

    const navMenuList = getGroupNavMenuList({
        router: useRouter(),
        activeId: id
    });

    const { titleText, 
            metaDescriptionText, 
            mainHeadingHtml,
            strapLineText } = content ?? {};

    return (

        <StandardLayout {...rest}>
            <Head>
                <title>{titleText}</title>
                <meta name="description" content={metaDescriptionText} />
            </Head>
            <LayoutColumnContainer>
                <PageHeader 
                    id="group"
                    content={{
                        mainHeadingHtml: mainHeadingHtml, 
                        descriptionHtml: strapLineText,
                        navMenuTitleText: 'Group menu'
                    }}
                    image={image}
                    navMenuList={navMenuList}
                    className="u-bg-theme-14" />
                {children}
            </LayoutColumnContainer>
        </StandardLayout>

    )
    
}