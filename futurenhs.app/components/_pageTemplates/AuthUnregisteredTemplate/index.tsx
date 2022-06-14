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

    const { mainHeading, bodyHtml } = contentText ?? {};

    return (

        <PageBody className="tablet:u-px-0">
            <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
            <RichText wrapperElementType="div" bodyHtml={bodyHtml} />
        </PageBody>

    )

}
