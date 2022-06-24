import { PageBody } from '@components/PageBody'
import { RichText } from '@components/RichText'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'

import { Props } from './interfaces'
import Link from 'next/link'

/**
 * Auth signout template
 */
export const AuthSignOutTemplate: (props: Props) => JSX.Element = ({
    routes,
    contentText,
}) => {
    const { authSignIn } = routes ?? {}
    const { mainHeading, intro, signIn } = contentText ?? {}

    return (
        <PageBody className="tablet:u-px-0">
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={8} desktop={6}>
                    {mainHeading && (
                        <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
                    )}
                    {intro && (
                        <RichText
                            wrapperElementType="div"
                            className="u-mb-8"
                            bodyHtml={intro}
                        />
                    )}
                    {authSignIn && signIn && (
                        <Link href={authSignIn}>
                            <a className="c-button">{signIn}</a>
                        </Link>
                    )}
                </LayoutColumn>
            </LayoutColumnContainer>
        </PageBody>
    )
}
