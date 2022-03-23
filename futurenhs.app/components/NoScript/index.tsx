import classNames from 'classnames';

import { RichText } from '@components/RichText';
import { Heading } from '@components/Heading';

import { Props } from './interfaces';

export const NoScript: (props: Props) => JSX.Element = ({
    headingLevel,
    text,
    className
}) => {

    const { heading, body } = text ?? {};

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-warning-callout', className)
    };

    return (

        <noscript >
            <div className={generatedClasses.wrapper}>
                <Heading level={headingLevel} className="nhsuk-warning-callout__label">
                    {heading}<span className="u-sr-only">:</span>
                </Heading>
                <RichText wrapperElementType="p" bodyHtml={body} />
            </div>
        </noscript>

    )

}
