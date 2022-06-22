import classNames from 'classnames'

import { RichText } from '@components/RichText'
import { Heading } from '@components/Heading'

import { Props } from './interfaces'

export const WarningCallout: (props: Props) => JSX.Element = ({
    headingLevel,
    text,
    className,
}) => {
    const { heading, body } = text ?? {}

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-warning-callout', className),
        heading: classNames('nhsuk-warning-callout__label')
    }

    return (
        <div className={generatedClasses.wrapper}>
            <Heading
                level={headingLevel}
                className={generatedClasses.heading}
            >
                {heading}
                <span className="u-sr-only">:</span>
            </Heading>
            <RichText wrapperElementType="p" bodyHtml={body} />
        </div>
    )
}
