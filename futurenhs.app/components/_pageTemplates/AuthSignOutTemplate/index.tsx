import { PageBody } from '@components/PageBody';

import { Props } from './interfaces'

/**
 * Auth signout template
 */
export const AuthSignOutTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    routes,
    contentText
}) => {

    const { authApiSignOut } = routes ?? {};
    const { mainHeading, signOut } = contentText ?? {};

    return (

        <PageBody className="tablet:u-px-0">
            <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
            <form action={authApiSignOut} method="POST" encType="multipart/form-data">
                <input type="hidden" name="csrfToken" value={csrfToken} />
                <input type="hidden" name="callbackUrl" value="http://localhost:5000" />
                <button type="submit" className="c-button">{signOut}</button>
            </form>
        </PageBody>

    )

}
