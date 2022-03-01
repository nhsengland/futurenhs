import classNames from 'classnames';
import Head from 'next/head';

import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { PageBody } from '@components/PageBody';

import { Props } from './interfaces';

export const GenericContentTemplate: (props: Props) => JSX.Element = ({
    user,
    contentText
}) => {

    const isAuthenticated: boolean = Boolean(user);

    const { metaDescription,
        title,
        mainHeading } = contentText ?? {};

    return (

        <>
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
        </>

    )

}
