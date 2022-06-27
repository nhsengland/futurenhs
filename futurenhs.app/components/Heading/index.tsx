import { Props } from './interfaces'

/**
 * Heading with configurable level. Useful for use within other components where the correct heading level may depend on the current composition.
 * Accepts components or strings as children.
 */
export const Heading: Function = ({
    level = 3,
    className,
    children,
}: Props) => {
    let clampedLevel: number = level

    /**
     * Clamp to valid HTML heading level
     */
    clampedLevel = level < 1 ? 1 : clampedLevel
    clampedLevel = level > 6 ? 6 : clampedLevel

    const HeadingTag: any = `h${clampedLevel}`

    return <HeadingTag className={className}>{children}</HeadingTag>
}
