import classNames from 'classnames';
import Head from 'next/head';

import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { PageBody } from '@components/PageBody';

import { Props } from './interfaces';

export const GenericContentTemplate: (props: Props) => JSX.Element = ({
    user,
    actions,
    contentText,
    className
}) => {

    const isAuthenticated: boolean = Boolean(user);

    const { metaDescription, 
            title, 
            mainHeading } = contentText ?? {};

    const generatedClasses: any = {
        wrapper: classNames('u-bg-theme-3', className)
    }

    return (

        <StandardLayout
            shouldRenderSearch={isAuthenticated}
            shouldRenderUserNavigation={isAuthenticated}
            shouldRenderMainNav={isAuthenticated}
            user={user}
            actions={actions}
            className={generatedClasses.wrapper}>
                <Head>
                    <title>{title}</title>
                    <meta name="description" content={metaDescription} />
                </Head>
                <LayoutColumnContainer>
                    <LayoutColumn>
                        <PageBody>
                            <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
                        </PageBody>
                    </LayoutColumn>
                </LayoutColumnContainer>
        </StandardLayout>

    )

}
