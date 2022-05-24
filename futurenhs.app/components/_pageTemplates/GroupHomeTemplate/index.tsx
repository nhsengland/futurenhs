import classNames from 'classnames'

import { themes } from '@constants/themes'
import { selectTheme } from '@selectors/themes'
import { LayoutColumn } from '@components/LayoutColumn'
import { Theme } from '@appTypes/theme'

import { Props } from './interfaces'
import { RichText } from '@components/RichText'

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
    themeId,
    contentText,
}) => {
    const { bodyHtml } = contentText ?? {}

    const { background }: Theme = selectTheme(themes, themeId)

    const generatedClasses: any = {
        wrapper: classNames('c-page-body'),
        adminCallOut: classNames(
            'nhsuk-inset-text u-m-0 u-pr-0 u-max-w-full',
            `u-border-l-theme-${background}`
        ),
        adminCallOutText: classNames('nhsuk-heading-m u-text-bold'),
        adminCallOutButton: classNames(
            'c-button c-button-outline c-button--min-width u-w-full u-drop-shadow u-mt-4 tablet:u-mt-0'
        ),
        previewButton: classNames(
            'c-button c-button-outline c-button--min-width u-w-full u-mt-4 u-drop-shadow tablet:u-mt-0 tablet:u-mr-5'
        ),
        publishButton: classNames(
            'c-button c-button--min-width u-w-full u-mt-4 tablet:u-mt-0'
        ),
    }

    return (
        <LayoutColumn tablet={12} className={generatedClasses.wrapper}>
            <RichText
                wrapperElementType="div"
                bodyHtml={bodyHtml}
                className="u-text-lead u-text-theme-7"
            />
        </LayoutColumn>
    )
}