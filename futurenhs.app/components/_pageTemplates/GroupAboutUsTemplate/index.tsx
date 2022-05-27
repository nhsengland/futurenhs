import { LayoutColumn } from '@components/LayoutColumn'
import { Props } from './interfaces'

export const GroupAboutUsTemplate: (props: Props) => JSX.Element = ({
    user,
    actions,
    contentText,
}) => {
    const { secondaryHeading } = contentText

    return (
        <>
            <LayoutColumn tablet={12} className="c-page-body">
                <h2>{secondaryHeading}</h2>
            </LayoutColumn>
        </>
    )
}
