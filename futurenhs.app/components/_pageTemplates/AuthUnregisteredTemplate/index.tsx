import { Link } from '@components/Link';
import { PageBody } from '@components/PageBody';
import { RichText } from '@components/RichText';

import { Props } from './interfaces'

/**
 * Auth unregistered template
 */
export const AuthUnregisteredTemplate: (props: Props) => JSX.Element = ({
    routes,
    contentText
}) => {

    const { authSignOut } = routes;
    const { mainHeading, bodyHtml, signOut } = contentText ?? {};

    return (

        <PageBody className="tablet:u-px-0">
            <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
            <RichText wrapperElementType="div" className="u-mb-10" bodyHtml={bodyHtml} />
            {signOut &&
                <Link href={authSignOut}>
                    <a className="c-button c-button-outline u-drop-shadow">{signOut}</a>
                </Link>
            }
        </PageBody>

    )

}
