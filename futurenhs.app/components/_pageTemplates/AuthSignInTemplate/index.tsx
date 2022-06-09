import { PageBody } from '@components/PageBody';

import { Props } from './interfaces'

/**
 * Auth signin template
 */
export const AuthSignInTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    routes,
    contentText
}) => {

    const { authApiSignInAzureB2C } = routes ?? {};
    const { mainHeading, signIn } = contentText ?? {};

    return (

        <PageBody className="tablet:u-px-0">
            <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
            <form action={authApiSignInAzureB2C} method="POST" encType="multipart/form-data">
                <input type="hidden" name="csrfToken" value={csrfToken} />
                <input type="hidden" name="callbackUrl" value="http://localhost:5000" />
                <button type="submit" className="c-button">{signIn}</button>
            </form>
        </PageBody>

    )

}
