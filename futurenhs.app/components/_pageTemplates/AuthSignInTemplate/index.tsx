import { PageBody } from '@components/PageBody'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { RichText } from '@components/RichText'

import { Props } from './interfaces'

/**
 * Auth signin template
 */
export const AuthSignInTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    routes,
    contentText,
}) => {
    const { authApiSignInAzureB2C } = routes ?? {}
    const { mainHeading, secondaryHeading, intro, bodyHtml, signIn } =
        contentText ?? {}

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
                    <form
                        action={authApiSignInAzureB2C}
                        method="POST"
                        encType="multipart/form-data"
                        className="u-mb-12"
                    >
                        <input
                            type="hidden"
                            name="csrfToken"
                            value={csrfToken}
                        />
                        <input
                            type="hidden"
                            name="callbackUrl"
                            value={process.env.APP_URL}
                        />
                        <button type="submit" className="c-button">
                            {signIn}
                        </button>
                    </form>
                    {secondaryHeading && (
                        <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                    )}
                    {bodyHtml && (
                        <RichText
                            wrapperElementType="div"
                            className="u-mb-10"
                            bodyHtml={bodyHtml}
                        />
                    )}
                </LayoutColumn>
            </LayoutColumnContainer>
        </PageBody>
    )
}
