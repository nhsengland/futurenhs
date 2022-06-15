import { PageBody } from '@components/PageBody';
import { RichText } from '@components/RichText';
import Link from 'next/link';

import { Props } from './interfaces'

/**
 * Auth signout template
 */
export const AuthSignOutTemplate: (props: Props) => JSX.Element = ({
    routes,
    contentText
}) => {

    const { authSignIn } = routes ?? {};
    const { mainHeading, bodyHtml, signIn } = contentText ?? {};

    return (

        <PageBody className="tablet:u-px-0">
            <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
            {bodyHtml &&
                <RichText wrapperElementType="div" className="u-mb-10" bodyHtml={bodyHtml} />
            }
            {signIn &&
                <Link href={authSignIn}>
                    <a className="c-button">{signIn}</a>
                </Link>
            }
        </PageBody>

    )

}
