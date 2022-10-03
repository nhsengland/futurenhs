import classNames from 'classnames'

import { RichText } from '@components/generic/RichText'
import { Heading } from '@components/layouts/Heading'

import { Props } from './interfaces'

/**
 * Derived from the NHS Design System Warning Callout component: https://service-manual.nhs.uk/design-system/components/warning-callout.
 * Used to help users identify and understand warning content on the page, even if they do not read the whole page.
 */
export const WarningCallout: (props: Props) => JSX.Element = ({
    headingLevel,
    text,
    className,
}) => {
    const { heading, body } = text ?? {}

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-warning-callout', className),
        heading: classNames('nhsuk-warning-callout__label'),
    }

    return (
        <div className={generatedClasses.wrapper}>
            <Heading level={headingLevel} className={generatedClasses.heading}>
                {heading}
                <span className="u-sr-only">:</span>
            </Heading>
            <RichText wrapperElementType="p" bodyHtml={body} />
        </div>
    )
}
